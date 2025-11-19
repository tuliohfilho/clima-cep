import React from 'react';
import './HistoryDetail.css';

/**
 * Formata CEP para XXXXX-XXX
 */
const formatCep = (cep) => {
  if (!cep) return '';
  const clean = cep.toString().replace(/\D/g, '');
  return `${clean.slice(0, 5)}-${clean.slice(5)}`;
};

/**
 * Formata data para formato relativo
 */
const getRelativeTime = (timestamp) => {
  const now = new Date();
  const date = new Date(timestamp);
  const seconds = Math.floor((now - date) / 1000);
  
  if (seconds < 60) return 'agora';
  if (seconds < 3600) return `há ${Math.floor(seconds / 60)} minuto(s)`;
  if (seconds < 86400) return `há ${Math.floor(seconds / 3600)} hora(s)`;
  if (seconds < 604800) return `há ${Math.floor(seconds / 86400)} dia(s)`;
  
  return date.toLocaleDateString('pt-BR');
};

/**
 * Componente para exibir detalhes de um item do histórico
 */
const HistoryDetail = ({ data, onClose }) => {
  if (!data) {
    return (
      <div className="empty-detail">
        <p>Selecione uma busca para ver os detalhes</p>
      </div>
    );
  }

  const formatCepNumber = (cep) => {
    if (!cep) return '';
    const clean = cep.toString().replace(/\D/g, '');
    return `${clean.slice(0, 5)}-${clean.slice(5)}`;
  };

  return (
    <div className="history-detail">
      <div className="detail-header">
        <h3>Detalhes da Busca</h3>
        <button className="btn-close" onClick={onClose}>✕</button>
      </div>

      <div className="detail-timestamp">
        {getRelativeTime(data.timestamp)}
      </div>

      <div className="detail-section">
        <h4>Informações do CEP</h4>
        <div className="detail-grid">
          <div className="detail-item">
            <span className="label">CEP:</span>
            <span className="value">{formatCepNumber(data.zipCode)}</span>
          </div>
          <div className="detail-item">
            <span className="label">Logradouro:</span>
            <span className="value">{data.street || 'Não informado'}</span>
          </div>
          <div className="detail-item">
            <span className="label">Bairro:</span>
            <span className="value">{data.district || 'Não informado'}</span>
          </div>
          <div className="detail-item">
            <span className="label">Cidade:</span>
            <span className="value">{data.city}</span>
          </div>
          <div className="detail-item">
            <span className="label">Estado:</span>
            <span className="value">{data.state}</span>
          </div>
          {data.ibge && (
            <div className="detail-item">
              <span className="label">IBGE:</span>
              <span className="value">{data.ibge}</span>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default HistoryDetail;
