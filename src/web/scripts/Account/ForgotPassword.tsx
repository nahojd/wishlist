import React, { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { resetPassword, resetPasswordForEmail, validatePwdResetToken } from "./Actions";
import { PageHeader } from "../Components/PageHeader";
import { NavLink, useNavigate } from "react-router";
import { getApiCallState, useStateSelector } from "../Model";
import { Alert } from "../Components/Alert";
import { clearApiCallState } from "../ApiCalls/Actions";
import { isValidEmail, isValidPassword, MinPwdLength } from "../Utils/Validation";

export const ForgotPasswordPage = () => {

	const apicalls = getApiCallState();
	const submitState = apicalls.getLoadingState("resetPasswordForEmail");
	const user = useStateSelector(state => state.account.user);
	const isAuthenticated = !!user;

	const dispatch = useDispatch();
	const navigate = useNavigate();

	const [email, setEmail] = useState("");

	const emailValid = isValidEmail(email);

	const submit = (e: React.FormEvent) => {
		if (emailValid)
			dispatch(resetPasswordForEmail(email));
		e.preventDefault();
	};

	useEffect(() => {
		if (isAuthenticated)
			navigate("/");
	}, [isAuthenticated]);

	useEffect(() => {
		return () => {
			if (submitState)
				dispatch(clearApiCallState("resetPasswordForEmail"));
		};
	}, []);

	return <>
		<PageHeader />

		<article>
			<header>Glömt lösenord</header>

			<Alert type="info" show={submitState !== "complete"}>
				Ange din e-postadress så kommer ett mail med instruktioner för att återställa lösenordet skickas till dig.
			</Alert>

			<form onSubmit={submit}>
				<fieldset>
					<label htmlFor="email">E-post</label>
					<input type="email" id="email" required onChange={e => setEmail(e.target.value)} value={email}
						aria-invalid={emailValid ? "false" : null}/>
				</fieldset>

				<Alert type="success" show={submitState === "complete"}>
					<>Ett mail med instruktioner har skickats till <strong>{email}</strong>.</>
				</Alert>
				<Alert type="danger" show={submitState === "failed"}><>
					<strong>Fel!</strong> Är du säker på att du angivit rätt e-postadress?
				</></Alert>

				{ submitState !== "complete" &&
					<button type="submit" aria-busy={submitState === "started"} disabled={!emailValid}>Skicka</button>}
			</form>

			<footer>
				<NavLink to="/login">Tillbaka till inloggningen</NavLink>
			</footer>
		</article>


	</>
};

export const ResetPasswordPage = () => {

	const dispatch = useDispatch();
	const apicalls = getApiCallState();
	const validateTokenState = apicalls.getLoadingState("validatePwdResetToken");
	const submitState = apicalls.getLoadingState("resetPassword");

	const [token, setToken] = useState<string>();
	const [password, setPassword] = useState("");
	const [invalidToken, setInvalidToken] = useState(false);

	const validPassword = isValidPassword(password);

	const submit = (e: React.FormEvent) => {
		if (validPassword)
			dispatch(resetPassword(token, password));
		e.preventDefault();
	};

	useEffect(() => {
		if (token || validateTokenState)
			return; //Då har vi redan läst ut den
		const queryParams = new URLSearchParams(location.search);
		const t = queryParams.get("token");
		setToken(t);
		if (!t)
			setInvalidToken(true);

	}, [location.search]);

	useEffect(() => {
		if (token)
			dispatch(validatePwdResetToken(token));
	}, [token]);

	useEffect(() => {
		if (validateTokenState === "failed")
			setInvalidToken(true);
	}, [validateTokenState]);

	return <>
		<PageHeader />

		<article>
			<header>Återställ lösenord</header>
			<Alert type="info" show={validateTokenState === "started"}>Kontrollerar token...</Alert>
			<Alert type="danger" show={invalidToken}>Ogiltig token!</Alert>

			{ validateTokenState === "complete" && <>

			<Alert type="info" show><>Ange nytt lösenord. Lösenordet måste vara minst {MinPwdLength} tecken långt.</></Alert>

			<form onSubmit={submit}>
				<fieldset>
					<label htmlFor="password">Lösenord</label>
					<input type="password" id="password" required onChange={e => setPassword(e.target.value)} value={password}
						aria-invalid={validPassword ? "false" : null} />
				</fieldset>


				<Alert type="success" show={submitState === "complete"}>
					<>Lösenordet är ändrat. Du kan nu <NavLink to="/login">logga in</NavLink> med det nya lösenordet.</>
				</Alert>
				<Alert type="danger" show={submitState === "failed"}><>
					<strong>Fel!</strong> Det gick inte att uppdatera lösenordet.
				</></Alert>

				<button type="submit" aria-busy={submitState === "started"} disabled={!validPassword}>Ändra lösenord</button>
			</form>
			</>}
		</article>

	</>
};