import React from "react";
import { PageHeader } from "../Components/PageHeader";
import { NavLink } from "react-router-dom";
import { useForm } from "react-hook-form";
import { getApiCallState } from "../Model";
import { useDispatch } from "react-redux";
import { registerUser } from "./Actions";
import { Alert } from "../Components/Alert";

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

	return <>
		<PageHeader />

		<article>
			<form onSubmit={handleSubmit(submit)}>
				<header>Registrera nytt konto</header>

				<fieldset>
					<label htmlFor="namn">Namn</label>
					<input type="text" id="namn" {...register("name", { required: true })} />

					<label htmlFor="email">E-post</label>
					<input type="email" id="email" {...register("email", { required: true })} />

					<label htmlFor="password">Lösenord</label>
					<input type="password" id="password" {...register("password", { required: true })} />
				</fieldset>

				<NavLink to="/login">Har du redan konto? Logga in</NavLink>
				<footer>
					<button aria-busy={submitState === "started"} type="submit">Skapa konto</button>
				</footer>
			</form>

			<Alert type="success" show={submitState === "complete"}>Ditt konto är registrerat, men ännu inte verifierat.</Alert>
			<Alert type="danger" show={submitState === "failed"}>Det gick inte att registrera ditt konto.</Alert>
		</article>
	</>
};