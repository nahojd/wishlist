import React, { useEffect } from "react"
import { Navigate, useNavigate, useParams } from "react-router-dom"
import { getApiCallState, useStateSelector } from "../Model";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { clearApiCallState } from "../ApiCalls/Actions";
import { Alert } from "../Components/Alert";
import { updateWish } from "../Actions";

export const EditWishPage = () => {

	const { id } = useParams<{ id: string }>();

	return <EditWish wishId={parseInt(id)} />
}


interface IEditWishFields {
	name: string;
	description: string;
	linkUrl: string;
};

export const EditWish = (props: { wishId: number }) => {

	const { register, handleSubmit } = useForm<IEditWishFields>();

	const navigate = useNavigate();
	const dispatch = useDispatch();

	const user = useStateSelector(state => state.account.user);
	const wishlists = useStateSelector(state => state.wishlist.users);
	const wishes = wishlists?.find(x => x.id === user?.id)?.wishes;
	const wish = wishes?.find(x => x.id === props.wishId);

	const apicalls = getApiCallState();
	const submitState = apicalls.getLoadingState("updateWish");

	const submit = (data: IEditWishFields) => {
		console.debug("submit", data);
		dispatch(updateWish(props.wishId, data));
	};

	useEffect(() => {
		return () => {
			dispatch(clearApiCallState("updateWish"));
		};
	}, []);

	useEffect(() => {
		if (submitState === "complete")
			navigate(`/user/${user.id}`);
	}, [submitState]);

	if (!user || (!!wishes && !wish))
		return <Navigate to="/" />;

	if (!wish)
		return <section>
			<Alert type="info" aria-busy="true">Hämtar önskning...</Alert>
		</section>

	return <section>
		<h1>Ändra önskning</h1>

		<form onSubmit={handleSubmit(submit)}>

			<fieldset>
				<label htmlFor="name">Namn</label>
				<input type="text" id="name" {...register("name", { required: true })} defaultValue={wish.name} />

				<label htmlFor="description">Beskrivning</label>
				<textarea rows={3} id="description" {...register("description", { required: false })} defaultValue={wish.description} />

				<label htmlFor="linkUrl">Länk</label>
				<input type="url" id="linkUrl" {...register("linkUrl", { required: false })} defaultValue={wish.linkUrl} />
			</fieldset>

			<Alert type="danger" show={submitState === "failed"}><><strong>Fel!</strong> Det gick inte att spara önskningen.</></Alert>

			<footer>
				<button type="submit" aria-busy={submitState === "started"}>Spara</button>
				<button className="outline secondary" onClick={() => navigate(`/user/${user.id}`)}>Avbryt</button>
			</footer>
		</form>
	</section>
}