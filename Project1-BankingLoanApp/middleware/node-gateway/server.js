// Node.js middleware layer (API gateway)
// React  ->  this gateway  ->  ASP.NET Core Web API  ->  SQL Server
//
// It does two jobs:
//   1. Routing: every request starting with /api is forwarded to the Web API
//   2. Logging: every request is written to the console (see logger.js)
// The JWT token in the Authorization header is passed through untouched,
// so the Web API still does all authentication and authorization.

require('dotenv').config();
const express = require('express');
const { createProxyMiddleware } = require('http-proxy-middleware');
const requestLogger = require('./logger');

const PORT = process.env.PORT || 4000;                 // Azure sets PORT itself
const API_URL = process.env.API_URL || 'http://localhost:5000';

const app = express();

app.use(requestLogger);

// Simple check that the gateway itself is running
app.get('/health', (req, res) => {
  res.json({ status: 'ok', gateway: 'running', forwardingTo: API_URL });
});

// Forward /api/* to the Web API (CORS is handled by the Web API)
app.use(
  createProxyMiddleware({
    target: API_URL,
    changeOrigin: true,
    pathFilter: '/api',
    on: {
      error: (err, req, res) => {
        console.error(`Proxy error for ${req.method} ${req.url}: ${err.message}`);
        res.writeHead(502, { 'Content-Type': 'application/json' });
        res.end(JSON.stringify({ statusCode: 502, message: 'The banking API is not reachable right now.' }));
      },
    },
  })
);

app.listen(PORT, () => {
  console.log(`Gateway running on port ${PORT}, forwarding /api to ${API_URL}`);
});
