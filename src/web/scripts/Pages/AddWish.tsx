import React, { useEffect } from "react"
import { Navigate, useNavigate } from "react-router"
import { getApiCallState, useStateSelector } from "../Model";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { addWish } from "../Actions";
import { clearApiCallState } from "../ApiCalls/Actions";
import { Alert } from "../Components/Alert";


interface IAddWishFields {
	name: string;
	description: string;
	linkUrl: string;
};

export const AddWishPage = () => {

	const { register, handleSubmit } = useForm<IAddWishFields>();

	const navigate = useNavigate();
	const dispatch = useDispatch();

	const user = useStateSelector(state => state.account.user);

	const apicalls = getApiCallState();
	const submitState = apicalls.getLoadingState("addWish");

	const submit = (data: IAddWishFields) => {
		console.debug("submit", data);
		dispatch(addWish(data));
	};

	useEffect(() => {
		return () => {
			dispatch(clearApiCallState("addWish"));
		};
	}, []);

	useEffect(() => {
		if (submitState === "complete")
			navigate(`/user/${user.id}`);
	}, [submitState]);

	if (!user)
		return <Navigate to="/" />;

	return <section>
		<h1>Ny önskning</h1>

		<form onSubmit={handleSubmit(submit)}>

			<fieldset>
				<label htmlFor="name">Namn</label>
				<input type="text" id="name" {...register("name", { required: true })} />

				<label htmlFor="description">Beskrivning</label>
				<textarea rows={3} id="description" {...register("description", { required: false })} />

				<label htmlFor="linkUrl">Länk</label>
				<input type="url" id="linkUrl" {...register("linkUrl", { required: false })} />
			</fieldset>

			<Alert type="danger" show={submitState === "failed"}><><strong>Fel!</strong> Det gick inte att spara önskningen.</></Alert>

			<footer>
				<button type="submit" aria-busy={submitState === "started"}>Spara</button>
				<button className="outline secondary" onClick={() => navigate(`/user/${user.id}`)}>Avbryt</button>
			</footer>
		</form>
	</section>
}