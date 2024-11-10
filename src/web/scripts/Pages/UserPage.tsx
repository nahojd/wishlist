import React, { useEffect } from "react";
import { Userlist } from "../Components/Userlist";
import { Navigate, useParams } from "react-router-dom";
import { getApiCallState, IUser, useStateSelector } from "../Model";
import { useDispatch } from "react-redux";
import { getUserWishes } from "../Actions";

export const UserPage = () => {
	const dispatch = useDispatch();

	const { id } = useParams<{ id: string }>();
	const currentUser = useStateSelector(state => state.account.user);
	const user = useStateSelector(state => state.wishlist.users?.find(x => x.id === parseInt(id)));
	const apicalls = getApiCallState();
	const getWishesState = apicalls.getLoadingState("getUserWishes");

	const isMyPage = currentUser.id === user?.id;

	useEffect(() => {
		if (user && getWishesState !== "started") {
			dispatch(getUserWishes(user.id));
		}
	}, [user?.id]);

	if (!user)
		return <Navigate to="/" />

	return <>
		<Userlist selectedUser={parseInt(id)} />
		<section>
			<h1>{user.name} {isMyPage ? "(jag själv)" : ""}</h1>

			{ getWishesState === "started" && <span aria-busy="true">Hämtar önskningar...</span> }

			<WishList user={user} />
		</section>
	</>;
}

export const WishList = (props: { user: IUser }) => {
	if (!props.user.wishes)
		return null;

	if (props.user.wishes.length === 0)
		return <article className="alert info">Det finns inga önskningar!</article>

	return <ul className="plain">
		{ props.user.wishes.map(x => <li key={x.id}>
			<article>
				<header>{x.name}</header>
				{x.description && <p>{x.description}</p>}
				{x.linkUrl && <a href={x.linkUrl} target="_blank">{x.linkUrl}</a>}
				<footer></footer>
			</article>

		</li>)}
	</ul>
}