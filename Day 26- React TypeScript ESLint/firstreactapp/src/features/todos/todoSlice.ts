import { createSlice, nanoid } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";

export interface Todo {
  id: string;
  text: string;
  completed: boolean;
}

const initialState: Todo[] = [];

const todoSlice = createSlice({
  name: "todos",
  initialState,
  reducers: {
    addTodo: {
      reducer: (state, action: PayloadAction<Todo>) => {
        state.push(action.payload);
      },
      prepare: (text: string) => ({
        payload: { id: nanoid(), text, completed: false },
      }),
    },
    toggleTodo: (state, action: PayloadAction<string>) => {
      const todo = state.find((t) => t.id === action.payload);
      if (todo) todo.completed = !todo.completed;
    },
    deleteTodo: (state, action: PayloadAction<string>) =>
      state.filter((t) => t.id !== action.payload),
    clearCompleted: (state) => state.filter((t) => !t.completed),
  },
});

export const { addTodo, toggleTodo, deleteTodo, clearCompleted } =
  todoSlice.actions;

export default todoSlice.reducer;