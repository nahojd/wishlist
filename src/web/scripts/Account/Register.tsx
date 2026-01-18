import React, { useEffect, useState } from "react";
import { PageHeader } from "../Components/PageHeader";
import { NavLink } from "react-router";
import { useForm } from "react-hook-form";
import { getApiCallState } from "../Model";
import { useDispatch } from "react-redux";
import { registerUser } from "./Actions";
import { Alert } from "../Components/Alert";
import { clearApiCallState } from "../ApiCalls/Actions";
import { isInvalidField, isValidEmail, isValidPassword } from "../Utils/Validation";
import Icon from "@mdi/react";
import { mdiEyeOffOutline, mdiEyeOutline } from "@mdi/js";

interface IRegisterFields {
	name: string;
	email: string;
	password: string;
	message: string;
}

export const RegisterPage = () => {
	const { register, handleSubmit, formState } = useForm<IRegisterFields>({ mode: "onBlur" });

	const dispatch = useDispatch();

	const apicalls = getApiCallState();
	const submitState = apicalls.getLoadingState("register");

	const [showPassword, setShowPassword] = useState(true);

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
					<Alert type="info" show><>
						Här kan du registrera ett konto, men det måste verifieras manuellt innan du kan använda Önskelistemaskinen.<br />
						Om du inte vet vem Johan är ska du nog inte registrera ett konto. Om du tror att Johan inte vet vem du är är det lämpligt att berätta det i meddelanderutan.
					</></Alert>

					<fieldset>
						<label htmlFor="namn">Namn</label>
						<input type="text" id="namn" {...register("name", { required: true })} aria-invalid={isInvalidField(formState, "name")} />

						<label htmlFor="email">E-post</label>
						<input type="email" id="email" {...register("email", { required: true, validate: isValidEmail })} aria-invalid={isInvalidField(formState, "email")} />

						<label htmlFor="password">Lösenord</label>
						<input type={showPassword ? "text" : "password"} id="password" {...register("password", { required: true, validate: isValidPassword })}
							placeholder="Lösenordet måste vara minst 8 tecken långt." aria-invalid={isInvalidField(formState, "password")} />
						<small>
							{showPassword ?
								<button className="link" type="button" onClick={() => setShowPassword(false)}><Icon path={mdiEyeOffOutline} /> Dölj lösenord</button> :
								<button className="link" type="button" onClick={() => setShowPassword(true)}><Icon path={mdiEyeOutline} /> Visa lösenord</button>
							}
						</small>

						<label htmlFor="message">Meddelande (frivilligt)</label>
						<textarea id="message" rows={3} {...register("message")} />

						<Alert type="danger" show={submitState === "failed"}><>
							Det gick inte att registrera ditt konto.<br />
							{apicalls.getError("register")}
						</></Alert>

						<button aria-busy={submitState === "started"} type="submit">Skapa konto</button>
					</fieldset>
				</form>
			</>}

			<Alert type="success" show={submitState === "complete"}>Ditt konto är registrerat, men ännu inte verifierat. Du kan inte logga in förrän kontot blivit verifierat.</Alert>


			<footer>
				<NavLink to="/login">Har du redan konto? Logga in</NavLink>
			</footer>



		</article>
	</>
};