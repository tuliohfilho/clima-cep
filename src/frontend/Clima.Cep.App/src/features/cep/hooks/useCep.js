import { useState, useCallback } from 'react';
import cepService from '../services/cepService';

/**
 * Hook para gerenciar a lógica de busca de CEP
 * Gerencia estado, loading, erros e histórico local
 */
const useCep = () => {
  const [cepData, setCepData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [searchHistory, setSearchHistory] = useState(() => {
    try {
      const saved = localStorage.getItem('cepSearchHistory');
      return saved ? JSON.parse(saved) : [];
    } catch {
      return [];
    }
  });

  /**
   * Busca CEP e atualiza histórico
   */
  const searchCep = useCallback(async (cep) => {
    if (!cep || cep.trim() === '') {
      setError('Por favor, insira um CEP válido');
      return null;
    }

    setLoading(true);
    setError(null);

    try {
      const cleanCep = cep.replace(/\D/g, '');
      if (cleanCep.length !== 8) {
        throw new Error('CEP deve conter 8 dígitos');
      }

      const data = await cepService.searchCep(cleanCep);
      setCepData(data);

      // Adicionar ao histórico local
      const newHistoryEntry = {
        id: new Date().getTime().toString(),
        zipCode: cleanCep,
        street: data.street,
        district: data.district,
        city: data.city,
        state: data.state,
        timestamp: new Date().toISOString()
      };

      const newHistory = [
        newHistoryEntry,
        ...searchHistory.filter(item => item.zipCode !== cleanCep)
      ].slice(0, 20);

      setSearchHistory(newHistory);
      localStorage.setItem('cepSearchHistory', JSON.stringify(newHistory));

      return data;
    } catch (err) {
      const errorMessage = err.message || 'Erro ao buscar CEP';
      setError(errorMessage);
      setCepData(null);
      return null;
    } finally {
      setLoading(false);
    }
  }, [searchHistory]);

  /**
   * Limpa os dados atuais
   */
  const clearData = useCallback(() => {
    setCepData(null);
    setError(null);
  }, []);

  /**
   * Remove um item do histórico
   */
  const removeFromHistory = useCallback((id) => {
    const newHistory = searchHistory.filter(item => item.id !== id);
    setSearchHistory(newHistory);
    localStorage.setItem('cepSearchHistory', JSON.stringify(newHistory));
  }, [searchHistory]);

  /**
   * Limpa todo o histórico
   */
  const clearHistory = useCallback(() => {
    setSearchHistory([]);
    localStorage.removeItem('cepSearchHistory');
  }, []);

  return {
    cepData,
    loading,
    error,
    searchHistory,
    searchCep,
    clearData,
    removeFromHistory,
    clearHistory
  };
};

export default useCep;
