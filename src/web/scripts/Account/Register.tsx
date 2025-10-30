import React, { useEffect } from "react";
import { PageHeader } from "../Components/PageHeader";
import { NavLink } from "react-router";
import { useForm } from "react-hook-form";
import { getApiCallState } from "../Model";
import { useDispatch } from "react-redux";
import { registerUser } from "./Actions";
import { Alert } from "../Components/Alert";
import { clearApiCallState } from "../ApiCalls/Actions";

interface IRegisterFields {
	name: string;
	email: string;
	password: string;
}

export const RegisterPage = () => {
	const { register, handleSubmit } = useForm<IRegisterFields>();

	const dispatch = useDispatch();

	const apicalls = getApiCallState();
	const submitState = apicalls.getLoadingState("register");

	const submit = (data: IRegisterFields) => {
		console.debug("submit", data);

		dispatch(registerUser(data));
	}

	useEffect(() => {
		return () => {
			dispatch(clearApiCallState("register"));
		};
	}, []);

	return <>
		<PageHeader />

		<article>
			<header>Registrera nytt konto</header>

			{ submitState !== "complete" && <>
				<form onSubmit={handleSubmit(submit)}>
					<Alert type="info" show>Här kan du registrera ett konto, men det måste verifieras manuellt innan du kan använda Önskelistemaskinen.</Alert>

					<fieldset>
						<label htmlFor="namn">Namn</label>
						<input type="text" id="namn" {...register("name", { required: true })} />

						<label htmlFor="email">E-post</label>
						<input type="email" id="email" {...register("email", { required: true })} />

						<label htmlFor="password">Lösenord</label>
						<input type="password" id="password" {...register("password", { required: true })} />

						<button aria-busy={submitState === "started"} type="submit">Skapa konto</button>
					</fieldset>
				</form>
			</>}

			<Alert type="success" show={submitState === "complete"}>Ditt konto är registrerat, men ännu inte verifierat. Du kan inte logga in förrän kontot blivit verifierat.</Alert>
			<Alert type="danger" show={submitState === "failed"}>Det gick inte att registrera ditt konto.</Alert>

			<footer>
				<NavLink to="/login">Har du redan konto? Logga in</NavLink>
			</footer>



		</article>
	</>
};