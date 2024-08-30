import React from "react";
import { useDispatch } from "react-redux";
import { logout } from "../Account/Actions";

export const Footer = () => {
	const dispatch = useDispatch();

	return <footer>
		<button className="outline" type="submit" onClick={() => dispatch(logout())}>Logga ut</button>
	</footer>;
}