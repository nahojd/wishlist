import React, { useState } from "react";
import { useDispatch } from "react-redux";
import { login } from "./Actions";
import { Button, Card, Form } from "react-bootstrap";
import { PageHeader } from "../app";

export const LoginPage = () => {

	const dispatch = useDispatch();

	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");

	const submit = () => {
		dispatch(login(email, password));
	};

	return <div className="container">
		<PageHeader />

		<Card>
			<Card.Header><Card.Title>Logga in</Card.Title></Card.Header>
			<Card.Body>
				<Form.Group>
					<Form.Label>E-post</Form.Label>
					<Form.Control type="email" onChange={e => setEmail(e.target.value)} value={email} />
				</Form.Group>

				<Form.Group>
					<Form.Label>Lösenord</Form.Label>
					<Form.Control type="email" onChange={e => setPassword(e.target.value)} value={password} />
				</Form.Group>
			</Card.Body>
			<Card.Footer>
				<Button variant="primary" type="submit" onClick={submit}>Logga in</Button>
			</Card.Footer>
		</Card>
	</div>
};