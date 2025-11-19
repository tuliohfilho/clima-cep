import { useState, useEffect, useCallback } from 'react';
import historyService from '../services/historyService';

/**
 * Hook para gerenciar o histórico de buscas (localStorage + API)
 */
const useHistory = () => {
  const [historyData, setHistoryData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [selectedItem, setSelectedItem] = useState(null);

  /**
   * Carrega o histórico do localStorage ou servidor
   */
  const loadHistory = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await historyService.getHistory();
      setHistoryData(Array.isArray(data) ? data : []);
    } catch (err) {
      // Não mostra erro, apenas usa o que tem localmente
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
      setError(null);
    } catch (err) {
      console.error('Erro ao remover item:', err);
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
      setError(null);
    } catch (err) {
      console.error('Erro ao limpar histórico:', err);
    }
  }, []);

  /**
   * Obtém detalhes de um item do histórico
   */
  const getDetail = useCallback(async (id) => {
    try {
      const data = await historyService.getHistoryDetail(id);
      setSelectedItem(data);
      setError(null);
    } catch (err) {
      console.error('Erro ao obter detalhes:', err);
    }
  }, []);

  /**
   * Adiciona item ao histórico
   */
  const addItem = useCallback(async (searchData) => {
    try {
      const result = await historyService.addToHistory(searchData);
      setHistoryData(prev => [result, ...prev].slice(0, 20));
      setError(null);
    } catch (err) {
      console.error('Erro ao adicionar ao histórico:', err);
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
    getDetail,
    addItem
  };
};

export default useHistory;
