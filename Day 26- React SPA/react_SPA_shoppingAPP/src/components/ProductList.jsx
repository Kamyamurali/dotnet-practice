function ProductList({ products }) {
  if (products.length === 0) return <p>No products found.</p>;

  return (
    <table>
      <thead>
        <tr><th>ID</th><th>Name</th><th>Price</th></tr>
      </thead>
      <tbody>
        {products.map((p) => (
          <tr key={p.productId}>
            <td>{p.productId}</td>
            <td>{p.productName}</td>
            <td>{p.price}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
export default ProductList;