import { FormState } from "react-hook-form";

export const MinPwdLength = 8;

export const isValidEmail = (email: string) => /^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+$/.test(email);

export const isValidPassword = (pwd: string) => !!pwd && pwd.length >= MinPwdLength;

export const isInvalidField = (formState: FormState<any>, field: string) => {
	if (!formState.dirtyFields[field])
		return null;
	return !!formState.errors[field];
};