import React from 'react';
import './HistoryList.css';

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
          >
            <div className="item-content" onClick={() => onSelectItem(item.id)}>
              <div className="item-main">
                <span className="item-cep">{item.cep}</span>
                <span className="item-city">{item.city}, {item.state}</span>
              </div>
              <div className="item-date">
                {new Date(item.timestamp).toLocaleDateString('pt-BR', {
                  day: '2-digit',
                  month: '2-digit',
                  year: 'numeric',
                  hour: '2-digit',
                  minute: '2-digit'
                })}
              </div>
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
          </li>
        ))}
      </ul>
    </div>
  );
};

export default HistoryList;
