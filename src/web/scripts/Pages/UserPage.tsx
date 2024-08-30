import React from "react";
import { Userlist } from "../Components/Userlist";
import { Navigate, useParams } from "react-router-dom";
import { useStateSelector } from "../app";

export const UserPage = () => {
	const { id } = useParams<{ id: string }>();

	const user = useStateSelector(state => state.wishlist.users?.find(x => x.id === parseInt(id)));

	if (!user)
		return <Navigate to="/" />

	return <>
		<Userlist selectedUser={parseInt(id)} />
		<section>
			<h1>{user.name}</h1>
		</section>
	</>;
}