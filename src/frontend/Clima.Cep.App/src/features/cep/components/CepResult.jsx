import React from 'react';
import './CepResult.css';

/**
 * Componente para exibir resultado da busca de CEP com clima
 */
const CepResult = ({ data, onClear }) => {
  if (!data) return null;

  const { location, weather } = data;

  return (
    <div className="cep-result">
      <button className="btn btn-secondary" onClick={onClear}>
        Fechar
      </button>

      <div className="result-grid">
        <div className="location-card">
          <h2>Localização</h2>
          {location && (
            <div className="location-info">
              <p><strong>CEP:</strong> {location.cep}</p>
              <p><strong>Rua:</strong> {location.street}</p>
              <p><strong>Bairro:</strong> {location.neighborhood}</p>
              <p><strong>Cidade:</strong> {location.city}</p>
              <p><strong>Estado:</strong> {location.state}</p>
              <p><strong>Latitude:</strong> {location.latitude}</p>
              <p><strong>Longitude:</strong> {location.longitude}</p>
            </div>
          )}
        </div>

        <div className="weather-card">
          <h2>Clima</h2>
          {weather && (
            <div className="weather-info">
              <div className="weather-main">
                <p className="temperature">{weather.temperature}°C</p>
                <p className="condition">{weather.condition}</p>
              </div>
              <div className="weather-details">
                <p><strong>Sensação Térmica:</strong> {weather.feels_like}°C</p>
                <p><strong>Umidade:</strong> {weather.humidity}%</p>
                <p><strong>Pressão:</strong> {weather.pressure} hPa</p>
                <p><strong>Vento:</strong> {weather.wind_speed} m/s</p>
                <p><strong>Cobertura de Nuvens:</strong> {weather.clouds}%</p>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default CepResult;
