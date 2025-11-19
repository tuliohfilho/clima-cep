import React, { useState, useEffect } from 'react';
import './CepSearchForm.css';

const CepSearchForm = ({ onSearch, loading, shouldClear }) => {
  const [cepInput, setCepInput] = useState('');

  useEffect(() => {
    if (shouldClear) {
      setCepInput('');
    }
  }, [shouldClear]);

  const handleInputChange = (e) => {
    let value = e.target.value.replace(/\D/g, '');
    if (value.length <= 8) {
      if (value.length <= 5) {
        setCepInput(value);
      } else {
        setCepInput(`${value.slice(0, 5)}-${value.slice(5)}`);
      }
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (cepInput) {
      onSearch(cepInput);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="cep-search-form">
      <div className="form-group">
        <label htmlFor="cepInput">Buscar por CEP</label>
        <input
          id="cepInput"
          type="text"
          placeholder="Digite o CEP (ex: 61765-350)"
          value={cepInput}
          onChange={handleInputChange}
          disabled={loading}
          maxLength="9"
          className="cep-input"
        />
      </div>
      <button type="submit" disabled={loading || !cepInput} className="search-btn">
        {loading ? 'Buscando...' : 'Buscar'}
      </button>
    </form>
  );
};

export default CepSearchForm;
