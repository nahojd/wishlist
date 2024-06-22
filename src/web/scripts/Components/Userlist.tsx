import React, { useEffect } from "react";
import { useDispatch } from "react-redux";
import { useStateSelector } from "../app";
import { getUsers } from "../Actions";

export const Userlist = () => {
	const dispatch = useDispatch();

	const users = useStateSelector(state => state.wishlist.users);

	useEffect(() => {
		if (!users)
			dispatch(getUsers());
	}, []);

	return <aside>
		<nav>
		{users && <ul>
			{users.map(x => <li key={x.id}>{x.name}</li>)}
		</ul>}
		</nav>
	</aside>;
};