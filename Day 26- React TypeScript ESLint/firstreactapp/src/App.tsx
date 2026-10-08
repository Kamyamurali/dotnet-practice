import { useState } from "react";
import Counter from "./features/counter/Counter";
import TodoList from "./features/todos/TodoList";

type Screen = "counter" | "todos";

function App() {
  const [screen, setScreen] = useState<Screen>("todos");

  return (
    <div className="app">
      <nav className="tabs">
        <button
          className={`tab ${screen === "counter" ? "active" : ""}`}
          onClick={() => setScreen("counter")}
        >
          Counter
        </button>
        <button
          className={`tab ${screen === "todos" ? "active" : ""}`}
          onClick={() => setScreen("todos")}
        >
          Todos
        </button>
      </nav>

      {screen === "counter" ? <Counter /> : <TodoList />}
    </div>
  );
}

export default App;