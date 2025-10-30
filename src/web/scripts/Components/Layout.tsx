import { Outlet } from "react-router";
import { Suspense } from "react";
import { PageHeader } from "./PageHeader";
import React from "react";
import { Footer } from "./Footer";

export const Layout = () => {
	return <>
		<main className="container">
			<PageHeader />
			<Suspense fallback={<div aria-busy="true">Loading...</div>}>
				<Outlet />
			</Suspense>
			<Footer />
		</main>
	</>;
};