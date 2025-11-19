import React from 'react';
import CepSearchForm from '../components/CepSearchForm';
import CepResult from '../components/CepResult';
import useCep from '../hooks/useCep';
import './CepPage.css';

/**
 * Página principal de busca por CEP
 */
const CepPage = () => {
  const {
    cepData,
    loading,
    error,
    searchCep,
    clearData
  } = useCep();

  return (
    <div className="cep-page">
      <div className="page-header">
        <h1>Buscar Informações de CEP e Clima</h1>
        <p>Insira um CEP para obter informações de localização e dados climáticos em tempo real</p>
      </div>

      <CepSearchForm onSearch={searchCep} loading={loading} />

      {error && (
        <div className="error-message">
          <strong>Erro:</strong> {error}
        </div>
      )}

      {loading && (
        <div className="loading-spinner">
          <div className="spinner"></div>
          <p>Carregando informações...</p>
        </div>
      )}

      {cepData && !loading && (
        <CepResult data={cepData} onClear={clearData} />
      )}
    </div>
  );
};

export default CepPage;
