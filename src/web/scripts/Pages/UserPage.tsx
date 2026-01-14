import React, { useEffect } from "react";
import { Friendslist } from "../Components/Friendslist";
import { Navigate, useNavigate, useParams, NavLink } from "react-router";
import { getApiCallState, IUser, IUserWish, useStateSelector } from "../Model";
import { useDispatch } from "react-redux";
import { avtjinga, deleteWish, getUserWishes, tjinga } from "../Actions";
import Icon from "@mdi/react";
import { mdiGift, mdiGiftOffOutline, mdiGiftOutline, mdiLinkVariant, mdiPencil, mdiPlus, mdiTrashCanOutline } from "@mdi/js";
import { getDomain } from "../Utils/Utils";

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
		<Friendslist selectedUser={parseInt(id)} />

		<section className="wishlist">
			<h1>{isMyPage ? "Min önskelista" : user.name}</h1>

			{ isMyPage && <p><small><button onClick={() => navigate("/wish/add")}><Icon path={mdiPlus} /> Ny önskning</button></small></p>}

			{ (!user.wishes || user.wishes.length === 0) && getWishesState === "started" && <span aria-busy="true">Hämtar önskningar...</span> }

			<WishList user={user} isOwnWish={isMyPage} />
		</section>
	</>;
}

const WishList = (props: { user: IUser, isOwnWish?: boolean }) => {

	const currentUser = useStateSelector(state => state.account.user);

	const dispatch = useDispatch();

	const deleteClicked = (wish: IUserWish) => {
		if (!props.isOwnWish)
			return;

		if (confirm(`Vill du ta bort önskningen "${wish.name}"`))
			dispatch(deleteWish(wish.id, props.user.id));
	};

	const tjingaClicked = (wish: IUserWish) => {
		dispatch(tjinga(wish.id));
	};

	const avtjingaClicked = (wish: IUserWish) => {
		dispatch(avtjinga(wish.id));
	};

	if (!props.user.wishes)
		return null;

	if (props.user.wishes.length === 0)
		return <article className="alert info">Det finns inga önskningar!</article>

	return <div>
		{ props.user.wishes.map(x => <article key={x.id}>
			<header>
				{ !x.tjingadBy && <Icon path={mdiGiftOutline} size={.75} /> }
				{ x.tjingadBy?.id === currentUser.id && <Icon path={mdiGift} size={.75} className="primary" /> }
				{ x.tjingadBy && x.tjingadBy?.id !== currentUser.id && <Icon path={mdiGiftOffOutline} size={.75} className="secondary" /> }
				{x.name}

				{/* { !props.isOwnWish && <>
					{ !x.tjingadBy && <button className="icon" aria-busy={x.pending} onClick={() => tjingaClicked(x)} title="Tjinga"><Icon path={mdiGift} /></button> }
					{ x.tjingadBy?.id === currentUser.id && <button className="icon" aria-busy={x.pending} onClick={() => avtjingaClicked(x)} title="Ta bort tjingning"><Icon path={mdiGiftOffOutline} /></button> }
				</>} */}
			</header>
			{x.description && <p>{x.description}</p>}
			{x.linkUrl && <><Icon path={mdiLinkVariant} size={.75} /> <a href={x.linkUrl} target="_blank">{getDomain(x.linkUrl)}</a></>}

			<footer>
				{ props.isOwnWish && <>
					<small><NavLink to={`/wish/${x.id}`} title="Ändra önskning" role="button" className="primary mr-1"><Icon path={mdiPencil} /> Ändra</NavLink></small>
					<small><button className="outline secondary" onClick={() => deleteClicked(x)}><Icon path={mdiTrashCanOutline} /> Ta bort</button></small>

				</> }
				{ !props.isOwnWish && <>
					{ !x.tjingadBy && <small><button onClick={() => tjingaClicked(x)} aria-busy={x.pending}>Tjinga</button></small> }
					{ x.tjingadBy?.id === currentUser.id && <small><button className="outline" onClick={() => avtjingaClicked(x)} aria-busy={x.pending}>Ta bort tjingning</button></small> }
					{ x.tjingadBy && x.tjingadBy.id !== currentUser.id && <em>Tjingad av {x.tjingadBy.name}</em>}
				</> }
			</footer>

		</article>)}
	</div>;
}