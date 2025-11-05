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

  <form-modal
    :is-open="isFormDialogOpen"
    submit-type="create"
    :loading="loading"
    :invalid="invalid"
    @submit="handleAdd"
    @cancel="handleCancel"
  >
    <domain-entity-types-form
      v-model:selected-entity-type-id="selectedEntityTypeId"
      :entity-types="availableEntityTypes"
    />
  </form-modal>

  <form-modal
    :is-open="isConfirmDialogOpen"
    submit-type="delete"
    :loading="loading"
    @submit="handleRemove"
    @cancel="confirmDialog.cancel"
  >
    <h2>Entiteitstype verwijderen uit {{ domain.name }}</h2>

    <p>
      Weet je zeker dat je entiteitstype <em>'{{ selectedEntityTypeName }}'</em> wilt verwijderen
      uit domein <em>'{{ domain.name }}'</em>?
    </p>
  </form-modal>
</template>

<script setup lang="ts">
import { computed, ref, type DeepReadonly } from "vue";
import ItemList from "@/components/ItemList.vue";
import FormModal from "@/components/FormModal.vue";
import IconContainer from "@/components/IconContainer.vue";
import { useDialog } from "@/composables/use-dialog";
import { useDomainEntityTypes } from "@/composables/use-domain-entity-types";
import type { DomainEntityTypes, EntityType } from "@/services/pabcService";
import DomainEntityTypesForm from "./DomainEntityTypesForm.vue";

const { domain, entityTypes } = defineProps<{
  domain: DeepReadonly<DomainEntityTypes>;
  entityTypes: DeepReadonly<EntityType[]>;
}>();

const emit = defineEmits<{ (e: "refresh"): void }>();

const { loading, invalid, addEntityTypeToDomain, removeEntityTypeFromDomain, clearInvalid } =
  useDomainEntityTypes();

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

const formDialog = useDialog();
const { isOpen: isFormDialogOpen } = formDialog;

const confirmDialog = useDialog();
const { isOpen: isConfirmDialogOpen } = confirmDialog;

const openAddDialog = () => {
  selectedEntityTypeId.value = "";

  formDialog.open();
};

const openRemoveDialog = (id: string) => {
  selectedEntityTypeId.value = id;

  confirmDialog.open();
};

const handleAdd = async () => {
  if (!domain.id) return;

  try {
    await addEntityTypeToDomain(domain.id, selectedEntityTypeId.value);

    formDialog.confirm();

    emit("refresh");
  } catch {
    // Error displayed via invalid, keep dialog open
  }
};

const handleCancel = () => {
  clearInvalid();

  formDialog.cancel();
};

const handleRemove = async () => {
  if (!domain.id) return;

  await removeEntityTypeFromDomain(domain.id, selectedEntityTypeId.value);

  confirmDialog.confirm();

  emit("refresh");
};
</script>
