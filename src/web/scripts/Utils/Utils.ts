export const getDomain = (linkUrl: string) => {
	const url = new URL(linkUrl);
	return url.hostname;
}