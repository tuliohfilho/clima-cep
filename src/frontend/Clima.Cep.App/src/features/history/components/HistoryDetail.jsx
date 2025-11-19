import React from 'react';
import './HistoryDetail.css';

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

  const { location, weather, timestamp } = data;

  return (
    <div className="history-detail">
      <div className="detail-header">
        <h3>Detalhes da Busca</h3>
        <button className="btn-close" onClick={onClose}>✕</button>
      </div>

      <div className="detail-timestamp">
        {new Date(timestamp).toLocaleDateString('pt-BR', {
          day: '2-digit',
          month: '2-digit',
          year: 'numeric',
          hour: '2-digit',
          minute: '2-digit',
          second: '2-digit'
        })}
      </div>

      {location && (
        <div className="detail-section">
          <h4>Localização</h4>
          <div className="detail-grid">
            <div className="detail-item">
              <span className="label">CEP:</span>
              <span className="value">{location.cep}</span>
            </div>
            <div className="detail-item">
              <span className="label">Rua:</span>
              <span className="value">{location.street}</span>
            </div>
            <div className="detail-item">
              <span className="label">Bairro:</span>
              <span className="value">{location.neighborhood}</span>
            </div>
            <div className="detail-item">
              <span className="label">Cidade:</span>
              <span className="value">{location.city}</span>
            </div>
            <div className="detail-item">
              <span className="label">Estado:</span>
              <span className="value">{location.state}</span>
            </div>
            <div className="detail-item">
              <span className="label">Coordenadas:</span>
              <span className="value">{location.latitude}, {location.longitude}</span>
            </div>
          </div>
        </div>
      )}

      {weather && (
        <div className="detail-section">
          <h4>Clima</h4>
          <div className="detail-grid">
            <div className="detail-item">
              <span className="label">Temperatura:</span>
              <span className="value">{weather.temperature}°C</span>
            </div>
            <div className="detail-item">
              <span className="label">Sensação Térmica:</span>
              <span className="value">{weather.feels_like}°C</span>
            </div>
            <div className="detail-item">
              <span className="label">Condição:</span>
              <span className="value">{weather.condition}</span>
            </div>
            <div className="detail-item">
              <span className="label">Umidade:</span>
              <span className="value">{weather.humidity}%</span>
            </div>
            <div className="detail-item">
              <span className="label">Pressão:</span>
              <span className="value">{weather.pressure} hPa</span>
            </div>
            <div className="detail-item">
              <span className="label">Vento:</span>
              <span className="value">{weather.wind_speed} m/s</span>
            </div>
            <div className="detail-item">
              <span className="label">Nuvens:</span>
              <span className="value">{weather.clouds}%</span>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default HistoryDetail;
