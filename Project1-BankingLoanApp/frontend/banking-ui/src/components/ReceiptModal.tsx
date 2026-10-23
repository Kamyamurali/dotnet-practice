import StatusBadge from './StatusBadge';
import { formatDateTime, formatMoney } from '../utils/format';
import type { PaymentReceipt } from '../types';

export default function ReceiptModal({ receipt, onClose }: { receipt: PaymentReceipt; onClose: () => void }) {
  const installments = receipt.installmentNos.length > 1
    ? `${receipt.installmentNos[0]} to ${receipt.installmentNos[receipt.installmentNos.length - 1]}`
    : String(receipt.installmentNos[0] ?? '—');

  const print = () => {
    document.body.classList.add('print-receipt');
    window.print();
    document.body.classList.remove('print-receipt');
  };

  return (
    <div className="modal" role="dialog" aria-modal="true" aria-labelledby="receipt-title">
      <div className="modal__box receipt">
        <div className="receipt__head">
          <h2 id="receipt-title">Payment receipt</h2>
          <StatusBadge status={receipt.status} />
        </div>
        <p className="receipt__amount">{formatMoney(receipt.amount)}</p>
        <dl className="facts">
          <div><dt>Transaction ID</dt><dd><strong>{receipt.transactionId}</strong></dd></div>
          <div><dt>Date and time</dt><dd>{formatDateTime(receipt.paidAt)}</dd></div>
          <div><dt>Status</dt><dd>{receipt.status === 'Success' ? 'Payment successful' : 'Reversed'}</dd></div>
          <div><dt>Payment type</dt><dd>{receipt.type === 'EarlyPayoff' ? 'Early loan repayment' : 'Monthly installment'}</dd></div>
          <div><dt>Installment(s)</dt><dd>{installments}</dd></div>
          <div><dt>Method</dt><dd>{receipt.method === 'NetBanking' ? 'Net Banking' : receipt.method === 'Branch' ? 'Paid at branch' : receipt.method}</dd></div>
          <div><dt>Loan</dt><dd>{receipt.productName} · #{receipt.applicationId}</dd></div>
          <div><dt>Customer</dt><dd>{receipt.customerName}</dd></div>
        </dl>
        <p className="hint">Demo receipt for a simulated payment. No real money was charged.</p>
        <div className="modal__actions no-print">
          <button className="btn btn-ghost" onClick={print}>Print</button>
          <button className="btn btn-primary" onClick={onClose}>Close</button>
        </div>
      </div>
    </div>
  );
}