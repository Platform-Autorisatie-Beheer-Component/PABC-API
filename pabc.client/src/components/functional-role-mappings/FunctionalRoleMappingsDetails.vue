<template>
  <details>
    <summary>
      {{ functionalRole.name }}
    </summary>

    <p v-if="!functionalRole.mappings.length">Geen mappings gevonden voor deze functionele rol.</p>

    <p v-else>Hieronder zie je de koppelingen die op deze functionele rol van toepassing zijn.</p>

    <item-list v-if="functionalRole.mappings.length" :items="functionalRole.mappings">
      <template #item="{ item: mapping }">
        <span>
          Is applicatierol <strong>{{ mapping.applicationRole }}</strong>
          {{
            !mapping.isAllEntityTypes
              ? `binnen domein ${mapping.domain}`
              : "voor alle entiteitstypes"
          }}
        </span>
      </template>
    </item-list>
  </details>
</template>

<script setup lang="ts">
import ItemList from "@/components/ItemList.vue";
import type { FunctionalRoleMappings } from "@/services/pabcService";

defineProps<{ functionalRole: FunctionalRoleMappings }>();
</script>
