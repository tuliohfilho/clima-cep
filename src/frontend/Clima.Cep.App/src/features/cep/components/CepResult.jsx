import React from 'react';
import './CepResult.css';

const CepResult = ({ data, onNewSearch }) => {
  if (!data) {
    return null;
  }

  const formatCep = (cep) => {
    return `${cep.slice(0, 5)}-${cep.slice(5)}`;
  };

  const resultItems = [
    { label: 'CEP Formatado', value: formatCep(data.zipCode) },
    { label: 'Logradouro', value: data.street || 'Não informado' },
    { label: 'Bairro', value: data.district || 'Não informado' },
    { label: 'Cidade', value: data.city },
    { label: 'UF', value: data.state },
    { label: 'Código IBGE', value: data.ibge || 'Não informado' },
    { label: 'Coordenadas', value: data.location ? `${data.location.latitude}, ${data.location.longitude}` : 'Não disponível' },
    { label: 'Provedor Utilizado', value: data.provider }
  ];

  return (
    <div className="cep-result">
      <h2>Resultado da Busca</h2>
      <div className="result-list">
        {resultItems.map((item, index) => (
          <div key={index} className="result-row">
            <span className="result-label">{item.label}</span>
            <span className="result-value">{item.value}</span>
          </div>
        ))}
      </div>
    </div>
  );
};

export default CepResult;
