<template>
  <details>
    <summary>
      {{ domain.name }}
    </summary>

    <p v-if="!domain.entityTypes?.length">Geen entiteitstypes gevonden voor dit domein.</p>

    <p v-else>Hieronder zie je de entiteitstypen die op '{{ domain.name }}' van toepassing zijn.</p>

    <item-list v-if="domain.entityTypes?.length" :items="domain.entityTypes">
      <template #item="{ item: entityType }">
        <span>
          <strong>{{ entityType.type }}</strong> | {{ entityType.name }}
        </span>
      </template>
    </item-list>
  </details>
</template>

<script setup lang="ts">
import ItemList from "@/components/ItemList.vue";
import type { DomainEntityTypes } from "@/services/pabcService";

const { domain } = defineProps<{ domain: DomainEntityTypes }>();
</script>
