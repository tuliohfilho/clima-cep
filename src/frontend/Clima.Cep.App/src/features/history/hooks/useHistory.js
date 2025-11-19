import { useState, useEffect, useCallback } from 'react';
import historyService from '../services/historyService';

/**
 * Hook para gerenciar o histórico de buscas
 */
const useHistory = () => {
  const [historyData, setHistoryData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [selectedItem, setSelectedItem] = useState(null);

  /**
   * Carrega o histórico do servidor
   */
  const loadHistory = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await historyService.getHistory();
      setHistoryData(data || []);
    } catch (err) {
      const errorMessage = err.message || 'Erro ao carregar histórico';
      setError(errorMessage);
      setHistoryData([]);
    } finally {
      setLoading(false);
    }
  }, []);

  /**
   * Remove um item do histórico
   */
  const removeItem = useCallback(async (id) => {
    try {
      await historyService.removeFromHistory(id);
      setHistoryData(prev => prev.filter(item => item.id !== id));
      if (selectedItem?.id === id) {
        setSelectedItem(null);
      }
    } catch (err) {
      setError(err.message || 'Erro ao remover item');
    }
  }, [selectedItem]);

  /**
   * Limpa todo o histórico
   */
  const clearAll = useCallback(async () => {
    try {
      await historyService.clearHistory();
      setHistoryData([]);
      setSelectedItem(null);
    } catch (err) {
      setError(err.message || 'Erro ao limpar histórico');
    }
  }, []);

  /**
   * Obtém detalhes de um item do histórico
   */
  const getDetail = useCallback(async (id) => {
    try {
      const data = await historyService.getHistoryDetail(id);
      setSelectedItem(data);
    } catch (err) {
      setError(err.message || 'Erro ao obter detalhes');
    }
  }, []);

  /**
   * Carrega histórico na montagem do componente
   */
  useEffect(() => {
    loadHistory();
  }, [loadHistory]);

  return {
    historyData,
    loading,
    error,
    selectedItem,
    setSelectedItem,
    loadHistory,
    removeItem,
    clearAll,
    getDetail
  };
};

export default useHistory;
