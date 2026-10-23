export const MINIMUM_AGE = 18;
export const MAX_INCOME_SHARE = 0.5; 

export function ageOn(dateOfBirth: string, today: Date = new Date()): number {
  const [y, m, d] = dateOfBirth.split('-').map(Number);
  let age = today.getFullYear() - y;
  const birthdayPassed =
    today.getMonth() + 1 > m || (today.getMonth() + 1 === m && today.getDate() >= d);
  if (!birthdayPassed) age--;
  return age;
}

export function checkEligibility(dateOfBirth: string, monthlyIncome: number, emi: number,
                                 today: Date = new Date()): string | null {
  if (!dateOfBirth) return 'Please enter your date of birth.';
  const age = ageOn(dateOfBirth, today);
  if (age < 0) return "Date of birth can't be in the future.";
  if (age < MINIMUM_AGE) return `You must be at least ${MINIMUM_AGE} years old to apply.`;
  if (age > 100) return 'Please check your date of birth.';
  if (!monthlyIncome || monthlyIncome <= 0) return 'Please enter your monthly income.';
  if (emi > monthlyIncome * MAX_INCOME_SHARE)
    return 'The monthly payment is more than 50% of your monthly income. Try a smaller amount or a longer tenure.';
  return null;
}