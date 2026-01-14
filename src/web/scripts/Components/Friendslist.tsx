import React, { useEffect, useMemo, useRef } from "react";
import { useDispatch } from "react-redux";
import { getUsers } from "../Actions";
import { NavLink } from "react-router";
import { useStateSelector } from "../Model";

export const Friendslist = (props: { selectedUser?: number }) => {
	const dispatch = useDispatch();

	const me = useStateSelector(state => state.account.user);
	const users = useStateSelector(state => state.wishlist.users);

	const friends = users?.filter(x => x.isFriend);
	const selectedUser = useMemo(() => users?.find(x => x.id === props.selectedUser), [props.selectedUser, users]);

	const detailsElement = useRef<HTMLDetailsElement>(null);

	useEffect(() => {
		if (!users)
			dispatch(getUsers());
	}, []);

	const closeDetails = () => {
		detailsElement.current?.removeAttribute("open");
	};

	return <aside>
		<nav className="friendlist">
		{friends && <>
			<details className="dropdown" ref={x => { detailsElement.current = x; }}>
				<summary role="button" className="primary outline">{selectedUser?.name || "Välj önskelista"}</summary>
				<ul>
					{props.selectedUser === me.id ?
						<li className="selected">{me.name}</li> :
						<li><NavLink to={`/user/${me.id}`} onClick={closeDetails}>{me.name}</NavLink></li>}
					{friends.map(x => x.id === props.selectedUser ?
						<li key={x.id} className="selected">{x.name}</li> :
						<li key={x.id}><NavLink to={`/user/${x.id}`} onClick={closeDetails}>{x.name}</NavLink></li>)}
				</ul>
			</details>
			<ul>
				{props.selectedUser === me.id ?
					<li className="selected">{me.name}</li> :
					<li><NavLink to={`/user/${me.id}`}>{me.name}</NavLink></li>}
				{friends.map(x => x.id === props.selectedUser ?
					<li key={x.id} className="selected">{x.name}</li> :
					<li key={x.id}><NavLink to={`/user/${x.id}`}>{x.name}</NavLink></li>)}
			</ul>
		</>}
		</nav>
	</aside>;
};