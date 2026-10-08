import { useSelector, useDispatch } from "react-redux";
import { increment, decrement, reset, incrementByAmount } from "./counterSlice";
import type { RootState, AppDispatch } from "../../app/store";
import "./Counter.css";

export default function Counter() {
  const count = useSelector((state: RootState) => state.counter.value);
  const dispatch = useDispatch<AppDispatch>();

  const tone = count > 0 ? "positive" : count < 0 ? "negative" : "zero";

  return (
    <div className="card">
      <p className="eyebrow">Redux Toolkit • React + TypeScript</p>
      <h1 className="title">Counter</h1>

      <div className={`value ${tone}`}>{count}</div>

      <div className="controls">
        <button className="btn round" onClick={() => dispatch(decrement())} aria-label="Decrement">−</button>
        <button className="btn round primary" onClick={() => dispatch(increment())} aria-label="Increment">+</button>
      </div>

      <div className="secondary">
        <button className="btn pill accent" onClick={() => dispatch(incrementByAmount(5))}>+5</button>
        <button className="btn pill ghost" onClick={() => dispatch(reset())}>Reset</button>
      </div>

     
    </div>
  );
}