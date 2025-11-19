import React, { useState } from 'react';
import useCep from '../hooks/useCep';
import CepSearchForm from '../components/CepSearchForm';
import CepResult from '../components/CepResult';
import './CepPage.css';

const CepPage = () => {
  const { cepData, loading, error, searchCep, clearSearch } = useCep();
  const [clearInput, setClearInput] = useState(false);

  const handleSearch = async (cep) => {
    await searchCep(cep);
    setClearInput(true);
  };

  const handleNewSearch = () => {
    clearSearch();
    setClearInput(true);
  };

  return (
    <div className="cep-page">
      <div className="cep-container">
        <h1>Busca de CEP</h1>
        <p className="subtitle">Consulte informações de localização por CEP</p>

        <CepSearchForm onSearch={handleSearch} loading={loading} shouldClear={clearInput} />

        {error && (
          <div className="error-message">
            <span>⚠️</span>
            <p>{error}</p>
          </div>
        )}

        {cepData && (
          <>
            <CepResult data={cepData} onNewSearch={handleNewSearch} />
          </>
        )}
      </div>
    </div>
  );
};

export default CepPage;
