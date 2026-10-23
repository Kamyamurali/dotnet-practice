
const PASSWORD_RULE = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$/;

export const emptyRegistration = { fullName: '', email: '', phone: '', password: '', confirm: '' };

export function validateRegistration(form: typeof emptyRegistration): string | null {
  if (form.fullName.trim().length < 2) return 'Please enter your full name.';
  if (!/^\S+@\S+\.\S+$/.test(form.email)) return 'Please enter a valid email address.';
  if (!PASSWORD_RULE.test(form.password))
    return 'Password must be at least 8 characters and include upper case, lower case, a number and a special character.';
  if (form.password !== form.confirm) return 'Passwords do not match.';
  return null;
}

