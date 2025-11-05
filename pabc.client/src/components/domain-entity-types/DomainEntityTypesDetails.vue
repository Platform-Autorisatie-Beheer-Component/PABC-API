<template>
  <details>
    <summary>
      {{ domain.name }}
    </summary>

    <p v-if="domain.description">{{ domain.description }}</p>

    <p v-if="!domain.entityTypes.length">Geen entiteitstypes gevonden voor dit domein.</p>

    <p v-else>Hieronder zie je de entiteitstypen die op '{{ domain.name }}' van toepassing zijn.</p>

    <p v-if="availableEntityTypes.length">
      <button type="button" class="button secondary" @click="openAddDialog">
        <icon-container icon="plus" /> Toevoegen
      </button>
    </p>

    <item-list
      v-if="domain.entityTypes.length"
      :items="linkedEntityTypes"
      @delete="openRemoveDialog"
    >
      <template #item="{ item: entityType }">
        <span>
          <strong>{{ entityType.type }}</strong> | {{ entityType.name }}
        </span>
      </template>
    </item-list>
  </details>

  <form-modal ref="form-dialog" submit-type="create" @submit="handleAdd">
    <domain-entity-types-form
      v-model:selected-entity-type-id="selectedEntityTypeId"
      :entity-types="availableEntityTypes"
    />
  </form-modal>

  <form-modal ref="confirm-dialog" submit-type="delete" @submit="handleRemove">
    <h2>Entiteitstype verwijderen uit {{ domain.name }}</h2>

    <p>
      Weet je zeker dat je entiteitstype <em>'{{ selectedEntityTypeName }}'</em> wilt verwijderen
      uit domein <em>'{{ domain.name }}'</em>?
    </p>
  </form-modal>
</template>

<script setup lang="ts">
import { computed, ref, useTemplateRef, type DeepReadonly } from "vue";
import ItemList from "@/components/ItemList.vue";
import FormModal from "@/components/FormModal.vue";
import IconContainer from "@/components/IconContainer.vue";
import { useDomainEntityTypes } from "@/composables/use-domain-entity-types";
import type { DomainEntityTypes, EntityType } from "@/services/pabcService";
import DomainEntityTypesForm from "./DomainEntityTypesForm.vue";

const { domain, entityTypes } = defineProps<{
  domain: DeepReadonly<DomainEntityTypes>;
  entityTypes: DeepReadonly<EntityType[]>;
}>();

const emit = defineEmits<{ (e: "refresh"): void }>();

const { addEntityTypeToDomain, removeEntityTypeFromDomain } = useDomainEntityTypes();

const selectedEntityTypeId = ref("");

const selectedEntityTypeName = computed(
  () => entityTypes.find((et) => et.id === selectedEntityTypeId.value)?.name
);

const linkedEntityTypes = computed(() =>
  entityTypes.filter((et) => et.id && domain.entityTypes.includes(et.id))
);

const availableEntityTypes = computed(() =>
  entityTypes.filter((et) => et.id && !domain.entityTypes.includes(et.id))
);

const formDialog = useTemplateRef("form-dialog");

const confirmDialog = useTemplateRef("confirm-dialog");

const openAddDialog = () => {
  selectedEntityTypeId.value = "";

  formDialog.value?.open();
};

const openRemoveDialog = (id: string) => {
  selectedEntityTypeId.value = id;

  confirmDialog.value?.open();
};

const handleAdd = async () => {
  if (!domain.id) return;
  await addEntityTypeToDomain(domain.id, selectedEntityTypeId.value);
  emit("refresh");
};

const handleRemove = async () => {
  if (!domain.id) return;
  await removeEntityTypeFromDomain(domain.id, selectedEntityTypeId.value);
  emit("refresh");
};
</script>
