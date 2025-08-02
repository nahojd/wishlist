import React, { useEffect } from "react";
import { Userlist } from "../Components/Userlist";
import { Navigate, useNavigate, useParams } from "react-router-dom";
import { getApiCallState, IUser, IUserWish, useStateSelector } from "../Model";
import { useDispatch } from "react-redux";
import { deleteWish, getUserWishes } from "../Actions";
import { NavLink } from "react-router-dom";

export const UserPage = () => {
	const dispatch = useDispatch();
	const navigate = useNavigate();

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

			{ isMyPage && <button onClick={() => navigate("/wish/add")}>Ny önskning</button>}

			{ (!user.wishes || user.wishes.length === 0) && getWishesState === "started" && <span aria-busy="true">Hämtar önskningar...</span> }

			<WishList user={user} allowEdit={isMyPage} />
		</section>
	</>;
}

const WishList = (props: { user: IUser, allowEdit?: boolean }) => {

	const dispatch = useDispatch();

	const deleteClicked = (wish: IUserWish) => {
		if (confirm(`Vill du ta bort önskningen "${wish.name}"`))
			dispatch(deleteWish(wish.id, props.user.id));
	};

	if (!props.user.wishes)
		return null;

	if (props.user.wishes.length === 0)
		return <article className="alert info">Det finns inga önskningar!</article>

	return <>
		<ul className="plain">
			{ props.user.wishes.map(x => <li key={x.id}>
				<article>
					<header>{props.allowEdit ? <NavLink to={`/wish/${x.id}`} title="Ändra önskning">{x.name}</NavLink> : x.name}</header>
					{x.description && <p>{x.description}</p>}
					{x.linkUrl && <a href={x.linkUrl} target="_blank">{x.linkUrl}</a>}
					{ props.allowEdit && <footer>
						<button className="secondary" onClick={() => deleteClicked(x)}>Ta bort</button>
					</footer> }

				</article>

			</li>)}
		</ul>
	</>;
}