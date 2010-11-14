function togglePassword(passwordFieldId, callerId) {
	var passwordField = document.getElementById(passwordFieldId);
	var mode = passwordField.getAttribute('type');
	var caller = document.getElementById(callerId);

	if (mode == 'password') {
		passwordField.setAttribute("type", "text");
		caller.innerHTML = 'Dölj tecken';
	}
	else {
		passwordField.setAttribute("type", "password");
		caller.innerHTML = 'Visa tecken';
	}
}