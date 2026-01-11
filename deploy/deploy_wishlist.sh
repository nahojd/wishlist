#!/bin/bash
set -e

# Usage: deploy_wishlist.sh /home/johan/wishlist/versions/<NEW_RELEASE_DIR>
if [ -z "$1" ]; then
  echo "Usage: $0 <new_release_directory>"
  exit 1
fi

NEW_RELEASE="$1"
CURRENT_SYMLINK="/home/johan/wishlist/current"
HEALTHCHECK_URL="https://wish-new.driessen.se/api/health"

echo "Deploying new release: $NEW_RELEASE"

# Step 1: Update the symlink to point to the new release
if [ -L "$CURRENT_SYMLINK" ]; then
  PREV_RELEASE=$(readlink -f "$CURRENT_SYMLINK")
else
  PREV_RELEASE=""
fi

echo "Previous release: $PREV_RELEASE"
ln -sfn "$NEW_RELEASE" "$CURRENT_SYMLINK"

# Step 2: Restart the application service
sudo systemctl restart kestrel-wishlist.service

# Step 3: Wait a few seconds for the app to start and perform health check
# Increase this if your app takes longer than 10 seconds to start up
# You may also want to perform this in a loop to make multiple attempts
sleep 10
HEALTH_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$HEALTHCHECK_URL")

if [ "$HEALTH_STATUS" -eq 200 ]; then
 echo "Health check passed: $HEALTH_STATUS"
else
 echo "Health check failed: $HEALTH_STATUS"
 if [ -n "$PREV_RELEASE" ]; then
   echo "Reverting to previous release..."
   sudo ln -sfn "$PREV_RELEASE" "$CURRENT_SYMLINK"
   sudo systemctl restart myapp.service
 else
   echo "No previous release available to revert to!"
 fi
 exit 1
fi

# Step 4: Cleanup - keep only the two most recent release directories
# You may modify this section of you want to keep more than the two most
# recent deployments on the server, or even if you would only like to keep the
# current one.
echo "Cleaning up old releases..."
# List directories sorted by modification time; exclude the 'current' symlink.
RELEASE_DIRS=( $(ls -1dt /home/johan/wishlist/versions/*/ 2>/dev/null | grep -v '/current/') )
if [ "${#RELEASE_DIRS[@]}" -gt 2 ]; then
  # Remove all but the two most recent directories
  for dir in "${RELEASE_DIRS[@]:2}"; do
    echo "Removing backup release: $dir"
    sudo rm -rf "$dir"
  done
fi

echo "Deployment successful."
exit 0