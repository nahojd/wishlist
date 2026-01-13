import React from "react";

export type AlertType = "success" | "warning" | "danger" | "info";

export const Alert = (props: { type: AlertType, show?: boolean; children?: React.ReactElement|string }) => {

	if (props.show === false)
		return null; //TODO: Lite snygg animation...

	return <article className={`alert ${props.type}`}>
		{props.children}
	</article>;
}