export const isValidEmail = (email: string) => /^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+$/.test(email);

export const isValidPassword = (pwd: string) => !!pwd && pwd.length >= 12;