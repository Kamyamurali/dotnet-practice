import { useState } from "react";
import type { FormEvent } from "react";
import { useSelector, useDispatch } from "react-redux";
import { addTodo, toggleTodo, deleteTodo, clearCompleted } from "./todoSlice";
import type { RootState, AppDispatch } from "../../app/store";
import "./TodoList.css";

type Filter = "all" | "active" | "completed";

export default function TodoList() {
  const todos = useSelector((state: RootState) => state.todos);
  const dispatch = useDispatch<AppDispatch>();

  // Local UI state: only this component needs it (slide 16)
  const [text, setText] = useState("");
  const [filter, setFilter] = useState<Filter>("all");

  const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const trimmed = text.trim();
    if (!trimmed) return;
    dispatch(addTodo(trimmed));
    setText("");
  };

  const visible = todos.filter((t) =>
    filter === "all" ? true : filter === "active" ? !t.completed : t.completed
  );
  const remaining = todos.filter((t) => !t.completed).length;

  return (
    <div className="card todo-card">
      <p className="eyebrow">Redux Toolkit • React + TypeScript</p>
      <h1 className="title">Todos</h1>

      <form className="todo-form" onSubmit={handleSubmit}>
        <input
          className="todo-input"
          value={text}
          onChange={(e) => setText(e.target.value)}
          placeholder="What needs doing?"
        />
        <button className="todo-add" type="submit">Add</button>
      </form>

      <div className="filters">
        {(["all", "active", "completed"] as Filter[]).map((f) => (
          <button
            key={f}
            className={`filter ${filter === f ? "active" : ""}`}
            onClick={() => setFilter(f)}
          >
            {f}
          </button>
        ))}
      </div>

      {visible.length === 0 ? (
        <p className="empty">
          {todos.length === 0 ? "No todos yet. Add one above." : "Nothing here."}
        </p>
      ) : (
        <ul className="todo-list">
          {visible.map((todo) => (
            <li key={todo.id} className={`todo-item ${todo.completed ? "done" : ""}`}>
              <label className="todo-label">
                <input
                  type="checkbox"
                  checked={todo.completed}
                  onChange={() => dispatch(toggleTodo(todo.id))}
                />
                <span>{todo.text}</span>
              </label>
              <button
                className="todo-delete"
                onClick={() => dispatch(deleteTodo(todo.id))}
                aria-label="Delete"
              >
                ×
              </button>
            </li>
          ))}
        </ul>
      )}

      {todos.length > 0 && (
        <div className="todo-footer">
          <span>{remaining} left</span>
          <button className="link" onClick={() => dispatch(clearCompleted())}>
            Clear completed
          </button>
        </div>
      )}
    </div>
  );
}