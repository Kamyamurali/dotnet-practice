import React from 'react'

class Login extends React.Component {
  render() {
    return (
      <div>
        <form>
        <h2>please log in</h2>
        <input type="text" placeholder="Username" />
        <input type="password" placeholder="Password" />
        <button>Login</button>
        </form>
      </div>
    );
  }
}

export default Login;
