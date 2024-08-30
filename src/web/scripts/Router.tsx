
import { Home } from "./Pages/Home";
import React from "react";
import { LoginPage } from "./Account/Login";
import { Layout } from "./Components/Layout";
import { Navigate } from "react-router-dom";
import { RegisterPage } from "./Account/Register";
import { UserPage } from "./Pages/UserPage";

export const publicRoutes = [
	{ path: "/login", element: <LoginPage /> },
	{ path: "/register", element: <RegisterPage /> },
	{ path: "*", element: <Navigate to="/login" replace /> }
];

export const privateRoute = {
	element: <Layout />,
	children: [
		{
			"path": "/",
			"element": <Home />
		},
		{
			"path": "/user/:id",
			"element": <UserPage />
		},
		{
			"path": "/wish/:id",
			"element": <div>Wish route</div>
		},
		{
			"path": "/profile",
			"element": <div>Profile path</div>
		},
		{ path: "*", element: <Navigate to="/" replace /> }
	]
}
;