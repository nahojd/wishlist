import { useDispatch } from "react-redux";
import { getApiCallState, useStateSelector } from "../Model";
import { useEffect } from "react";
import React from "react";
import { getShoppingList } from "../Actions";
import Icon from "@mdi/react";
import { mdiLinkVariant } from "@mdi/js";
import { getDomain } from "../Utils/Utils";
import { Alert } from "./Alert";

export const ShoppingList = () => {
	const dispatch = useDispatch();

	const wishes = useStateSelector(state => state.wishlist?.shoppingList);
	const loadState = getApiCallState().getLoadingState("getShoppingList");

	useEffect(() => {
		if (!loadState)
			dispatch(getShoppingList());
	}, [loadState]);

	return <>
		{wishes?.length > 0 && <table>
			<thead>
				<tr>
					<th scope="col">Önskning</th>
					<th scope="col">Önskad av</th>
				</tr>
			</thead>
			<tbody>
				{ wishes.map(x => <tr key={x.id}>
					<td scope="row">
						<strong role="heading">{x.name}</strong><br />
						{x.description}
						{x.linkUrl && <><br /><Icon path={mdiLinkVariant} size={.75} /> <a href={x.linkUrl} target="_blank">{getDomain(x.linkUrl)}</a></>}
					</td>
					<td style={{verticalAlign: "top"}}>{x.owner.name}</td>
				</tr>)}
			</tbody>
		</table>}

		{wishes && wishes.length === 0 && <Alert type="info">Du har inte tjingat några önskningar. Klicka på ett namn i listan för att börja tjinga!</Alert>}
	</>
}