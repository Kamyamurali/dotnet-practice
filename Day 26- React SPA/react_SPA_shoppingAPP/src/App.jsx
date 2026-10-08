import { useState, useEffect } from "react";
import { Routes, Route } from "react-router-dom";
import "./App.css";

import Navbar from "./components/Navbar";
import ProductList from "./components/ProductList";
import SearchProduct from "./components/SearchProduct";
import AddProduct from "./components/AddProduct";
import UpdateProduct from "./components/UpdateProduct";
import DeleteProduct from "./components/DeleteProduct";

const API_URL = "http://localhost:5000/api/Products"; // change to your API's port

function App() {
  const [products, setProducts] = useState([]);

  useEffect(() => {
    fetch(API_URL)
      .then((res) => {
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return res.json();
      })
      .then((data) => setProducts(data))
      .catch((err) => console.error("Failed to load products:", err));
  }, []);

  return (
    <div>
      <h1>Shopping app</h1>
      <Navbar />
<Routes>
  <Route path="/" element={<h2>Welcome to the Shopping App</h2>} />
  <Route path="/products" element={<ProductList products={products} />} />
  <Route path="/searchproduct" element={<SearchProduct />} />
  <Route path="/addproduct" element={<AddProduct />} />
  <Route path="/updateproduct" element={<UpdateProduct />} />
  <Route path="/deleteproduct" element={<DeleteProduct />} />
</Routes>
    </div>
  );
}
export default App;