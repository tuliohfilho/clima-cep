import React from 'react';
import './HistoryList.css';

/**
 * Formata data para formato relativo (ex: "há 2 minutos")
 */
const getRelativeTime = (timestamp) => {
  const now = new Date();
  const date = new Date(timestamp);
  const seconds = Math.floor((now - date) / 1000);
  
  if (seconds < 60) return 'agora';
  if (seconds < 3600) return `há ${Math.floor(seconds / 60)} min`;
  if (seconds < 86400) return `há ${Math.floor(seconds / 3600)}h`;
  if (seconds < 604800) return `há ${Math.floor(seconds / 86400)}d`;
  
  return date.toLocaleDateString('pt-BR');
};

/**
 * Formata CEP para XXXXX-XXX
 */
const formatCep = (cep) => {
  if (!cep) return '';
  const clean = cep.toString().replace(/\D/g, '');
  return `${clean.slice(0, 5)}-${clean.slice(5)}`;
};

/**
 * Componente para exibir lista de histórico
 */
const HistoryList = ({ items, onSelectItem, onRemoveItem, selectedId }) => {
  if (!items || items.length === 0) {
    return (
      <div className="empty-history">
        <p>Nenhuma busca realizada ainda</p>
      </div>
    );
  }

  return (
    <div className="history-list">
      <h3>Histórico de Buscas ({items.length})</h3>
      <ul className="history-items">
        {items.map((item) => (
          <li
            key={item.id}
            className={`history-item ${selectedId === item.id ? 'active' : ''}`}
            onClick={() => onSelectItem(item)}
          >
            <div className="item-header">
              <div className="item-main">
                <div className="item-cep">{formatCep(item.zipCode)}</div>
                <div className="item-city">{item.city}, {item.state}</div>
              </div>
              <div className="item-date">
                {getRelativeTime(item.timestamp)}
              </div>
            </div>
            <div className="item-footer">
              <div className="item-street">
                {item.street || 'Sem logradouro'}
              </div>
              <button
                className="btn-remove"
                onClick={(e) => {
                  e.stopPropagation();
                  onRemoveItem(item.id);
                }}
                title="Remover"
              >
                ✕
              </button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default HistoryList;
