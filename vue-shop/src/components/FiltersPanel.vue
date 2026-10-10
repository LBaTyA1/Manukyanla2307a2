<script setup>
defineProps({
  search: { type: String, required: true },
  category: { type: String, required: true },
  sort: { type: String, required: true },
  categories: { type: Array, required: true },
})

// v-model:search, v-model:category, v-model:sort в родителе
defineEmits(['update:search', 'update:category', 'update:sort'])
</script>

<template>
  <div class="filters">
    <input
      class="filters__search"
      type="search"
      placeholder="🔍 Поиск товара..."
      :value="search"
      @input="$emit('update:search', $event.target.value)"
    />

    <select
      class="filters__select"
      :value="category"
      @change="$emit('update:category', $event.target.value)"
    >
      <option value="all">Все категории</option>
      <option v-for="item in categories" :key="item" :value="item">{{ item }}</option>
    </select>

    <select
      class="filters__select"
      :value="sort"
      @change="$emit('update:sort', $event.target.value)"
    >
      <option value="default">Без сортировки</option>
      <option value="asc">Цена: по возрастанию</option>
      <option value="desc">Цена: по убыванию</option>
      <option value="name">По названию (А-Я)</option>
    </select>
  </div>
</template>

<style scoped>
.filters {
  display: grid;
  grid-template-columns: 1fr 220px 220px;
  gap: 12px;
  margin-bottom: 24px;
}
.filters__search,
.filters__select {
  padding: 12px 14px;
  border: 1px solid var(--border);
  border-radius: 10px;
  font-size: 15px;
  background: #fff;
  outline: none;
  font-family: inherit;
}
.filters__search:focus,
.filters__select:focus {
  border-color: var(--accent);
}
@media (max-width: 760px) {
  .filters {
    grid-template-columns: 1fr;
  }
}
</style>
