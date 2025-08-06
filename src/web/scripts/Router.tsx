
import { Home } from "./Pages/Home";
import React from "react";
import { LoginPage } from "./Account/Login";
import { Layout } from "./Components/Layout";
import { Navigate } from "react-router-dom";
import { RegisterPage } from "./Account/Register";
import { UserPage } from "./Pages/UserPage";
import { ForgotPasswordPage, ResetPasswordPage } from "./Account/ForgotPassword";
import { AddWishPage } from "./Pages/AddWish";
import { EditWishPage } from "./Pages/EditWish";

export const publicRoutes = [
	{ path: "/login", element: <LoginPage /> },
	{ path: "/register", element: <RegisterPage /> },
	{ path: "/forgotpassword", element: <ForgotPasswordPage /> },
	{ path: "/resetpassword", element: <ResetPasswordPage /> },
	{ path: "*", element: <Navigate to="/login" replace /> }
];

export const privateRoute = {
	element: <Layout />,
	children: [
		{ "path": "/", "element": <Home /> },
		{ "path": "/user/:id", "element": <UserPage /> },
		{ "path": "/wish/add", "element": <AddWishPage /> },
		{ "path": "/wish/:id", "element": <EditWishPage /> },
		{ "path": "/profile", "element": <div>Profile path</div> },
		{ path: "*", element: <Navigate to="/" replace /> }
	]
}
;