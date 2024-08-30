import React from "react";
import { PageHeader } from "../Components/PageHeader";
import { NavLink } from "react-router-dom";

export const RegisterPage = () => {
	return <>
		<PageHeader />

		<article>
			<header>Registrera nytt konto</header>
				<form>
					<fieldset>
						<label htmlFor="namn">Namn</label>
						<input type="text" id="namn" required />

						<label htmlFor="email">E-post</label>
						<input type="email" id="email" required />

						<label htmlFor="password">Lösenord</label>
						<input type="password" id="password" required />
					</fieldset>
				</form>

				<NavLink to="/login">Har du redan konto? Logga in</NavLink>
			<footer>
				<button type="submit">Skapa konto</button>
			</footer>
		</article>
	</>
};