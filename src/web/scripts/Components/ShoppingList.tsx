import { useDispatch } from "react-redux";
import { getApiCallState, useStateSelector } from "../Model";
import { useEffect } from "react";
import React from "react";
import { getShoppingList } from "../Actions";
import Icon from "@mdi/react";
import { mdiOpenInNew } from "@mdi/js";

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
						{x.linkUrl && <><br /><a href={x.linkUrl} target="_blank">{getDomain(x.linkUrl)} <Icon path={mdiOpenInNew} size={1} /></a></>}
					</td>
					<td style={{verticalAlign: "top"}}>{x.owner.name}</td>
				</tr>)}
			</tbody>
		</table>}
	</>
}

const getDomain = (linkUrl: string) => {
	const url = new URL(linkUrl);
	return url.hostname;
}