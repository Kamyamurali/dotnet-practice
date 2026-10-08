import { NavLink } from "react-router-dom";

function Navbar() {
  return (
    <nav className="navbar">
      <NavLink to="/">Home</NavLink>
      <NavLink to="/products">Products</NavLink>
      <NavLink to="/searchproduct">Search</NavLink>
      <NavLink to="/addproduct">Add Product</NavLink>
      <NavLink to="/updateproduct">Update Product</NavLink>
      <NavLink to="/deleteproduct">Delete Product</NavLink>
    </nav>
  );
}
export default Navbar;