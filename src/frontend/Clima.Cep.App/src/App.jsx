import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import CepPage from './features/cep/pages/CepPage';
import HistoryPage from './features/history/pages/HistoryPage';
import './App.css';

function App() {
  return (
    <Router>
      <div className="app">
        <nav className="navbar">
          <div className="navbar-content">
            <h1>Clima CEP</h1>
            <ul className="nav-links">
              <li><a href="/">Buscar CEP</a></li>
              <li><a href="/history">Histórico</a></li>
            </ul>
          </div>
        </nav>

        <main className="main-content">
          <Routes>
            <Route path="/" element={<CepPage />} />
            <Route path="/history" element={<HistoryPage />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </main>

        <footer className="footer">
          <p>&copy; 2024 Clima CEP. Todos os direitos reservados.</p>
        </footer>
      </div>
    </Router>
  );
}

export default App;
