import React, { Fragment, useEffect, useMemo, useState } from "react"
import { useForm } from "react-hook-form";
import { getApiCallState, IUser, useStateSelector } from "../Model";
import { useDispatch } from "react-redux";
import { Alert } from "../Components/Alert";
import { clearApiCallState } from "../ApiCalls/Actions";
import { updatePassword, updateUserSettings } from "../Account/Actions";
import { isInvalidField, isValidEmail, isValidPassword, MinPwdLength } from "../Utils/Validation";
import { getUsers, toggleFriendStatus } from "../Actions";


export const ProfilePage = () => {



	return <>
		<aside>
			<UserList />
		</aside>
		<section>
			<UserSettings />
			<ChangePassword />

		</section>
	</>
}

const UserList = () => {

	const dispatch = useDispatch();

	const currentUser = useStateSelector(state => state.account.user);
	const users = useStateSelector(state => state.wishlist.users);

	const submitState = getApiCallState().getLoadingState("toggleFriendStatus");

	useEffect(() => {
		if (!users)
			dispatch(getUsers());
	}, []);

	const toggleFriend = (user: IUser) => {
		dispatch(toggleFriendStatus(user.id));
	};

	return <article>
		{ users?.length > 0 && <fieldset>
			{users.filter(x => x.id != currentUser.id).map(x => <label key={x.id}>
				<input type="checkbox" name={`cbUser${x.id}`} checked={x.isFriend} onChange={() => toggleFriend(x)} disabled={submitState === "started"} aria-busy={submitState === "started"} />
				{x.name}
			</label>)}
		</fieldset>}
	</article>
}

const ChangePassword = () => {

	const dispatch = useDispatch();

	const { register, handleSubmit, formState } = useForm<{ password: string }>({ mode: "onBlur" });

	const apicalls = getApiCallState();
	const submitState = apicalls.getLoadingState("updatePassword");

	const [showPassword, setShowPassword] = useState(true);

	useEffect(() => {
		return () => {
			dispatch(clearApiCallState("updatePassword"));
		}
	}, []);

	const submit = (data: { password: string }) => {
		console.debug("submit", data);
		dispatch(updatePassword(data.password));
	}

	return <article>
		<header>Ändra lösenord</header>
		<form onSubmit={handleSubmit(submit)}>
			<fieldset role="group">
				<input type={showPassword ? "text" : "password"} name="password" placeholder="Ange nytt lösenord"
					{...register("password", { required: true, validate: isValidPassword })} aria-invalid={isInvalidField(formState, "password")} />
				<button type="submit" aria-busy={submitState === "started"}>Spara</button>
			</fieldset>
			<small>
				{showPassword ?
					<button className="link" type="button" onClick={() => setShowPassword(false)}>Dölj lösenord</button> :
					<button className="link" type="button" onClick={() => setShowPassword(true)}>Visa lösenord</button>
				}
			</small>
			{ formState.errors["password"] && <p>Lösenordet måste vara minst {MinPwdLength} tecken långt.</p>}

			<Alert type="success" show={submitState === "complete"}>Lösenordet är ändrat.</Alert>
			<Alert type="danger" show={submitState === "failed"}><><strong>Fel!</strong> Det gick inte att spara det nya lösenordet.</></Alert>
		</form>
	</article>;
}

const UserSettings = () => {

	const { register, handleSubmit, formState } = useForm<{ name: string, email: string, notify: boolean }>();
	const dispatch = useDispatch();

	const userState = useStateSelector(state => state.account.user);

	const apicalls = getApiCallState();
	const submitState = apicalls.getLoadingState("updateUserSettings");

	const submitErrors = useMemo(() => {
		if (submitState === "failed")
		{
			const errors = apicalls.getError("updateUserSettings");
			if (typeof(errors) === "string")
				return [errors];
			return Object.getOwnPropertyNames(errors).map(key => errors[key].join(", "));
		}
		return [];
	}, [submitState]);

	useEffect(() => {
		return () => {
			dispatch(clearApiCallState("updateUserSettings"));
		}
	}, []);

	const submit = (data: { name: string, email: string, notify: boolean }) => {
		console.debug("user settings", data);
		dispatch(updateUserSettings(data));
	}

	return <article>
		<header>Ändra uppgifter</header>

		<form onSubmit={handleSubmit(submit)}>
			<fieldset>
				<label htmlFor="namn">Namn</label>
				<input type="text" id="namn" defaultValue={userState.name} {...register("name", { required: true })} aria-invalid={isInvalidField(formState, "name")} />

				<label htmlFor="email">E-post</label>
				<input type="email" id="email" defaultValue={userState.email} {...register("email", { required: true, validate: isValidEmail })} aria-invalid={isInvalidField(formState, "email")} />

				<label>
					<input name="notify" type="checkbox" role="switch" {...register("notify")} defaultChecked={userState.notify} />
					Meddela mig via e-post när en önskning jag tjingat ändras
				</label>
			</fieldset>

			<Alert type="success" show={submitState === "complete"}>Uppgifterna är uppdaterade.</Alert>
			<Alert type="danger" show={submitState === "failed"}><>
				{submitErrors?.map(x => <Fragment key={x}>{x}<br /></Fragment>) || "Det gick inte att spara uppgifterna."}
			</></Alert>

			<footer>
				<button aria-busy={submitState === "started"} type="submit">Spara uppgifter</button>
			</footer>
		</form>
	</article>;
}