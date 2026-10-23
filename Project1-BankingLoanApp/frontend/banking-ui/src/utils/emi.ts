// EMI = P * r * (1 + r)^n / ((1 + r)^n - 1), where r is the monthly interest rate
export function calculateEmi(principal: number, yearlyRate: number, months: number): number {
  if (principal <= 0 || months <= 0) return 0;
  const r = yearlyRate / 12 / 100;
  if (r === 0) return round2(principal / months);
  const growth = Math.pow(1 + r, months);
  return round2((principal * r * growth) / (growth - 1));
}

const round2 = (n: number) => Math.round(n * 100) / 100;
