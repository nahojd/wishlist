import React, { useEffect } from "react";
import { useDispatch } from "react-redux";
import { getUsers } from "../Actions";
import { NavLink } from "react-router";
import { useStateSelector } from "../Model";

export const Friendslist = (props: { selectedUser?: number }) => {
	const dispatch = useDispatch();

	const users = useStateSelector(state => state.wishlist.users);

	const friends = users?.filter(x => x.isFriend);

	useEffect(() => {
		if (!users)
			dispatch(getUsers());
	}, []);

	return <aside>
		<nav>
		{friends && <ul>
			{friends.map(x => x.id === props.selectedUser ?
				<li key={x.id} className="selected">{x.name}</li> :
				<li key={x.id}><NavLink to={`/user/${x.id}`}>{x.name}</NavLink></li>)}
		</ul>}
		</nav>
	</aside>;
};